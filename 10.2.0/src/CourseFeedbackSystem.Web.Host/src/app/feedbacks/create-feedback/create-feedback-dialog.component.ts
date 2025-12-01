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
    selector: 'app-create-feedback-dialog',
    templateUrl: './create-feedback-dialog.component.html',
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
export class CreateFeedbackDialogComponent extends AppComponentBase implements OnInit {
    @Output() onSave = new EventEmitter<any>();

    saving = false;
    feedback: CreateUpdateFeedbackDto;
    courses: CourseDto[] = [];
    selectedFile: File | null = null;
    uploadProgress = 0;
    isUploading = false;

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
        this.initFeedback();
    }

    ngOnInit(): void {
        this.loadCourses();
    }

    private initFeedback(): void {
        this.feedback.id = 0;
        this.feedback.studentName = '';
        this.feedback.courseId = 0;
        this.feedback.comment = '';
        this.feedback.rating = null; // No default value
        this.feedback.fileUrl = '';
    }

    loadCourses(): void {
        this._courseService.getAll('', undefined, '', 0, 1000).subscribe({
            next: (result) => {
                Promise.resolve().then(() => {
                    this.courses = result.items.filter(c => c.isActive);
                    this.cd.detectChanges();
                });
            },
            error: (error) => {
                console.error('Error loading courses:', error);
                this.notify.error(this.l('FailedToLoadCourses'));
            }
        });
    }

    onCourseChange(event: any): void {
        console.log('Course changed:', event);
        console.log('Current feedback.courseId:', this.feedback.courseId);
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

            this.http.post(uploadUrl, formData).subscribe({
                next: (response: any) => {
                    this.isUploading = false;
                    this.uploadProgress = 0;

                    if (response && response.fileUrl) {
                        resolve(response.fileUrl);
                    } else {
                        this.notify.error(this.l('InvalidResponseFromServer'));
                        reject(new Error('Invalid response format'));
                    }
                },
                error: (error) => {
                    this.isUploading = false;
                    this.uploadProgress = 0;
                    this.notify.error(this.l('FileUploadFailed'));
                    reject(error);
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
            // Upload file if selected
            if (this.selectedFile) {
                try {
                    const fileUrl = await this.uploadFile();
                    if (fileUrl) {
                        this.feedback.fileUrl = fileUrl;
                    }
                } catch (error) {
                    this.saving = false;
                    console.error('File upload failed:', error);
                    return;
                }
            }

            // Ensure Id is 0 for create
            this.feedback.id = 0;

            console.log('Creating feedback - Full Payload:', JSON.stringify(this.feedback));
            console.log('CourseId being sent:', this.feedback.courseId);

            // Create the feedback
            this._feedbackService.create(this.feedback)
                .pipe(finalize(() => {
                    this.saving = false;
                    this.cd.detectChanges();
                }))
                .subscribe({
                    next: (result) => {
                        console.log('Feedback created successfully:', result);
                        this.notify.info(this.l('SavedSuccessfully'));
                        this.bsModalRef.hide();
                        this.onSave.emit();
                    },
                    error: (error) => {
                        console.error('Error creating feedback:', error);
                        this.handleFeedbackError(error);
                    }
                });

        } catch (error) {
            this.saving = false;
            console.error('Exception in save:', error);
            this.notify.error(this.l('SaveFailed'));
            this.cd.detectChanges();
        }
    }

    private handleFeedbackError(error: any): void {
        let errorMessage = this.l('SaveFailed');

        if (error.error) {
            try {
                const errorObj = error.error;
                errorMessage = errorObj.error?.message || errorObj.error?.details || errorObj.message || errorMessage;
            } catch (e) {
                console.error('Error parsing error response:', e);
            }
        }

        this.notify.error(errorMessage);
    }

    cancel(): void {
        this.bsModalRef.hide();
    }
}