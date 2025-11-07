import { Component, Injector, OnInit, EventEmitter, Output, ChangeDetectorRef } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { FeedbackServiceProxy, CreateUpdateFeedbackDto, CourseServiceProxy, CourseDto } from '@shared/service-proxies/service-proxies';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { CommonModule } from '@angular/common';
import { finalize } from 'rxjs/operators';

@Component({
    selector: 'app-edit-feedback-dialog',
    templateUrl: './edit-feedback-dialog.component.html',
    standalone: true,
    imports: [
        FormsModule,
        CommonModule,
        AbpModalHeaderComponent,
        AbpValidationSummaryComponent,
        AbpModalFooterComponent,
        LocalizePipe,
    ],
})
export class EditFeedbackDialogComponent extends AppComponentBase implements OnInit {
    @Output() onSave = new EventEmitter<any>();

    saving = false;
    feedback: CreateUpdateFeedbackDto;
    courses: CourseDto[] = [];
    selectedFile: File | null = null;
    uploadProgress = 0;
    isUploading = false;
    id: number;

    constructor(
        injector: Injector,
        public _feedbackService: FeedbackServiceProxy,
        public _courseService: CourseServiceProxy,
        public bsModalRef: BsModalRef,
        private http: HttpClient,
        private cd: ChangeDetectorRef
    ) {
        super(injector);
        this.feedback = new CreateUpdateFeedbackDto();
    }

    ngOnInit(): void {
        this.loadCourses();
        this._feedbackService.get(this.id).subscribe((result) => {
            this.feedback = result;
        });
    }

    loadCourses(): void {
        this._courseService.getAll('', undefined, '', 0, 1000).subscribe((result) => {
            this.courses = result.items.filter(c => c.isActive);
        });
    }

    onFileSelected(event: any): void {
        const file = event.target.files[0];
        if (file) {
            const allowedTypes = ['application/pdf', 'image/jpeg', 'image/jpg', 'image/png'];
            if (!allowedTypes.includes(file.type)) {
                this.notify.error(this.l('InvalidFileType'));
                return;
            }
            if (file.size > 10 * 1024 * 1024) {
                this.notify.error(this.l('FileSizeExceedsLimit'));
                return;
            }
            this.selectedFile = file;
        }
    }

    uploadFile(): Promise<string | null> {
        return new Promise((resolve, reject) => {
            if (!this.selectedFile) {
                resolve(null);
                return;
            }

            this.isUploading = true;
            this.uploadProgress = 50;

            const formData = new FormData();
            formData.append('file', this.selectedFile);

            const uploadUrl = '/FileUpload/UploadFile';
            
            console.log('📤 Uploading file to:', uploadUrl);

            this.http.post(uploadUrl, formData)
                .subscribe({
                    next: (response: any) => {
                        this.isUploading = false;
                        this.uploadProgress = 0;
                        
                        console.log('📥 File upload response:', response);
                        
                        if (response && response.fileUrl) {
                            console.log('✅ File uploaded successfully:', response.fileUrl);
                            resolve(response.fileUrl);
                        } else {
                            console.error('❌ Invalid response format:', response);
                            this.notify.error(this.l('InvalidResponseFromServer'));
                            reject(new Error('Invalid response format'));
                        }
                        this.cd.detectChanges();
                    },
                    error: (error) => {
                        this.isUploading = false;
                        this.uploadProgress = 0;
                        console.error('❌ File upload error:', error);
                        
                        let errorMessage = this.l('FileUploadFailed');
                        
                        if (error.status === 404) {
                            errorMessage = 'File upload endpoint not found. Please check if the server is running.';
                        } else if (error.status === 413) {
                            errorMessage = 'File is too large. Maximum size is 10MB.';
                        } else if (error.status === 415) {
                            errorMessage = 'File type not supported. Please upload PDF, JPG, or PNG files only.';
                        } else if (error.error && typeof error.error === 'string') {
                            try {
                                const errorObj = JSON.parse(error.error);
                                errorMessage = errorObj.message || errorObj.error?.message || errorMessage;
                            } catch (e) {
                                errorMessage = error.error;
                            }
                        } else if (error.error && error.error.message) {
                            errorMessage = error.error.message;
                        }
                        
                        this.notify.error(errorMessage);
                        reject(error);
                        this.cd.detectChanges();
                    }
                });
        });
    }

    async save(): Promise<void> {
        // Validation
        if (!this.feedback.studentName || this.feedback.studentName.trim() === '') {
            this.notify.warn(this.l('StudentNameRequired'));
            return;
        }

        if (!this.feedback.courseId || this.feedback.courseId === 0) {
            this.notify.warn(this.l('CourseRequired'));
            return;
        }

        if (!this.feedback.rating || this.feedback.rating < 1 || this.feedback.rating > 5) {
            this.notify.warn(this.l('RatingRequired'));
            return;
        }

        this.saving = true;

        try {
            let fileUrl = '';

            // Upload file if selected
            if (this.selectedFile) {
                try {
                    fileUrl = await this.uploadFile() || '';
                    this.feedback.fileUrl = fileUrl;
                    console.log('📝 Setting fileUrl on feedback:', fileUrl);
                } catch (error) {
                    this.saving = false;
                    console.error('File upload failed:', error);
                    return;
                }
            }

            // Update the feedback
            this._feedbackService.update(this.feedback)
                .pipe(finalize(() => {
                    this.saving = false;
                    this.cd.detectChanges();
                }))
                .subscribe({
                    next: (result) => {
                        console.log('✅ Feedback updated successfully:', result);
                        this.notify.info(this.l('SavedSuccessfully'));
                        this.bsModalRef.hide();
                        this.onSave.emit();
                    },
                    error: (error) => {
                        console.error('❌ Error updating feedback:', error);
                        this.handleFeedbackError(error);
                    }
                });

        } catch (error) {
            this.saving = false;
            console.error('❌ Exception in save:', error);
            this.notify.error(this.l('SaveFailed'));
            this.cd.detectChanges();
        }
    }

    private handleFeedbackError(error: any): void {
        let errorMessage = this.l('SaveFailed');
        
        if (error.error) {
            const errorObj = error.error;
            
            if (errorObj.error) {
                errorMessage = errorObj.error.message || errorObj.error.details || errorMessage;
            } else if (errorObj.message) {
                errorMessage = errorObj.message;
            }
        }
        
        this.notify.error(errorMessage);
    }

    cancel(): void {
        this.bsModalRef.hide();
    }
}