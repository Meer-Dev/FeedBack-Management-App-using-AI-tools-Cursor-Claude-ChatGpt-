import { ChangeDetectorRef, Component, Injector, OnInit, ViewChild } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase } from 'shared/paged-listing-component-base';
import { Table, TableModule } from 'primeng/table';
import { LazyLoadEvent, PrimeTemplate } from 'primeng/api';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { FormsModule } from '@angular/forms';
import { NgIf, NgFor, DatePipe } from '@angular/common';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { FeedbackServiceProxy, FeedbackDto, FeedbackDtoPagedResultDto, CourseServiceProxy, CourseDto } from '@shared/service-proxies/service-proxies';
import { CreateFeedbackDialogComponent } from './create-feedback/create-feedback-dialog.component';
import { EditFeedbackDialogComponent } from './edit-feedback/edit-feedback-dialog.component';

@Component({
    templateUrl: './feedbacks.component.html',
    animations: [appModuleAnimation()],
    standalone: true,
    imports: [FormsModule, TableModule, PrimeTemplate, NgIf, NgFor, PaginatorModule, LocalizePipe, DatePipe],
})
export class FeedbacksComponent extends PagedListingComponentBase<FeedbackDto> implements OnInit {
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    feedbacks: FeedbackDto[] = [];
    courses: CourseDto[] = [];
    keyword = '';
    courseId: number | null = null;
    minRating: number | null = null;
    maxRating: number | null = null;
    advancedFiltersVisible = false;

    constructor(
        injector: Injector,
        private _feedbackService: FeedbackServiceProxy,
        private _courseService: CourseServiceProxy,
        private _modalService: BsModalService,
        cd: ChangeDetectorRef
    ) {
        super(injector, cd);
    }

    ngOnInit(): void {
        this.loadCourses();
        this.refresh();
    }

    loadCourses(): void {
        // ✅ FIX: Use undefined instead of true, wrap in Promise
        this._courseService.getAll('', undefined, '', 0, 1000).subscribe((result) => {
            Promise.resolve().then(() => {
                this.courses = result.items;
                this.cd.detectChanges();
            });
        });
    }

    createFeedback(): void {
        this.showCreateOrEditFeedbackDialog();
    }

    editFeedback(feedback: FeedbackDto): void {
        this.showCreateOrEditFeedbackDialog(feedback.id);
    }

    clearFilters(): void {
        this.keyword = '';
        this.courseId = null;
        this.minRating = null;
        this.maxRating = null;
        this.refresh();
    }

    list(event?: LazyLoadEvent): void {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            if (this.primengTableHelper.records && this.primengTableHelper.records.length > 0) {
                return;
            }
        }
    
        this.primengTableHelper.showLoadingIndicator();
    
        this._feedbackService
            .getAll(
                this.keyword || '',
                this.courseId !== null ? this.courseId : undefined,
                this.minRating !== null ? this.minRating : undefined,
                this.maxRating !== null ? this.maxRating : undefined,
                this.primengTableHelper.getSorting(this.dataTable) || '',
                this.primengTableHelper.getSkipCount(this.paginator, event) || 0,
                this.primengTableHelper.getMaxResultCount(this.paginator, event) || 10
            )
            .pipe(
                finalize(() => {
                    this.primengTableHelper.hideLoadingIndicator();
                })
            )
            .subscribe({
                next: (result: FeedbackDtoPagedResultDto) => {
                    // ✅ FIX: Wrap in Promise to avoid change detection error
                    Promise.resolve().then(() => {
                        this.primengTableHelper.records = result.items;
                        this.primengTableHelper.totalRecordsCount = result.totalCount;
                        this.primengTableHelper.hideLoadingIndicator();
                        this.cd.detectChanges();
                    });
                },
                error: (err) => {
                    this.primengTableHelper.hideLoadingIndicator();
                    console.error('Error loading feedbacks:', err);
                    abp.notify.error(this.l('AnErrorOccurred'));
                    this.cd.detectChanges();
                }
            });
    }

    delete(feedback: FeedbackDto): void {
        abp.message.confirm(
            this.l('FeedbackDeleteWarningMessage', feedback.studentName),
            undefined,
            (result: boolean) => {
                if (result) {
                    this._feedbackService.delete(feedback.id).subscribe(() => {
                        abp.notify.success(this.l('SuccessfullyDeleted'));
                        this.refresh();
                    });
                }
            }
        );
    }

    downloadFile(fileUrl: string): void {
        if (!fileUrl) {
            this.notify.warn(this.l('NoFileAvailable'));
            return;
        }
    
        try {
            // If it's a relative URL, make sure it's accessible
            if (fileUrl.startsWith('/')) {
                // Use the download endpoint for better security and handling
                const downloadUrl = `/FileUpload/DownloadFile?fileUrl=${encodeURIComponent(fileUrl)}`;
                window.open(downloadUrl, '_blank');
            } else {
                // Absolute URL
                window.open(fileUrl, '_blank');
            }
        } catch (error) {
            console.error('Error downloading file:', error);
            this.notify.error(this.l('FileDownloadFailed'));
        }
    }

    private showCreateOrEditFeedbackDialog(id?: number): void {
        let createOrEditFeedbackDialog: BsModalRef;
        if (!id) {
            createOrEditFeedbackDialog = this._modalService.show(CreateFeedbackDialogComponent, {
                class: 'modal-lg',
            });
        } else {
            createOrEditFeedbackDialog = this._modalService.show(EditFeedbackDialogComponent, {
                class: 'modal-lg',
                initialState: {
                    id: id,
                },
            });
        }

        createOrEditFeedbackDialog.content.onSave.subscribe(() => {
            this.refresh();
        });
    }
}