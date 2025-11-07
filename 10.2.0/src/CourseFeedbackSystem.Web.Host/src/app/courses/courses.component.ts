import { ChangeDetectorRef, Component, Injector, OnInit, ViewChild } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase } from 'shared/paged-listing-component-base';
import { Table, TableModule } from 'primeng/table';
import { LazyLoadEvent, PrimeTemplate } from 'primeng/api';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { FormsModule } from '@angular/forms';
import { NgIf } from '@angular/common';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { CourseDtoPagedResultDto } from '@shared/service-proxies/service-proxies';
import { EditCourseDialogComponent } from './edit-course/edit-course-dialog.component';
import { CourseServiceProxy, CourseDto } from '@shared/service-proxies/service-proxies';
import { CreateCourseDialogComponent } from './create-course/create-course-dialog.component'; 
@Component({
    templateUrl: './courses.component.html',
    animations: [appModuleAnimation()],
    standalone: true,
    imports: [FormsModule, TableModule, PrimeTemplate, NgIf, PaginatorModule, LocalizePipe],
})
export class CoursesComponent extends PagedListingComponentBase<CourseDto> implements OnInit {
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    courses: CourseDto[] = [];
    keyword = '';
    isActive: boolean | null = null;
    advancedFiltersVisible = false;

    constructor(
        injector: Injector,
        private _courseService: CourseServiceProxy,
        private _modalService: BsModalService,
        private cdr: ChangeDetectorRef,
    ) {
        super(injector, cdr);
        this.cdr = cdr;
    }

    ngOnInit(): void {
        this.refresh();
    }

    createCourse(): void {
        this.showCreateOrEditCourseDialog();
    }

    editCourse(course: CourseDto): void {
        this.showCreateOrEditCourseDialog(course.id);
    }

    clearFilters(): void {
        this.keyword = '';
        this.isActive = null;
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
    
        // Provide default values instead of null/undefined
        this._courseService
            .getAll(
                this.keyword || '',
                this.isActive !== null ? this.isActive : undefined,
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
                next: (result: CourseDtoPagedResultDto) => {
                    this.primengTableHelper.records = result.items;
                    this.primengTableHelper.totalRecordsCount = result.totalCount;
                    this.primengTableHelper.hideLoadingIndicator();
                    this.cd.detectChanges();
                    this.cdr.detectChanges();
                },
                error: (err) => {
                    this.primengTableHelper.hideLoadingIndicator();
                    console.error('Error loading courses:', err);
                    abp.notify.error(this.l('AnErrorOccurred'));
                    this.cd.detectChanges();
                    this.cdr.detectChanges();
                }
            });
    }

    delete(course: CourseDto): void {
        abp.message.confirm(
            this.l('CourseDeleteWarningMessage', course.courseName),
            undefined,
            (result: boolean) => {
                if (result) {
                    this._courseService.delete(course.id).subscribe(() => {
                        abp.notify.success(this.l('SuccessfullyDeleted'));
                        this.refresh();
                    });
                }
            }
        );
    }

    private showCreateOrEditCourseDialog(id?: number): void {
        let createOrEditCourseDialog: BsModalRef;
        if (!id) {
            createOrEditCourseDialog = this._modalService.show(CreateCourseDialogComponent, {
                class: 'modal-lg',
            });
        } else {
            createOrEditCourseDialog = this._modalService.show(EditCourseDialogComponent, {
                class: 'modal-lg',
                initialState: {
                    id: id,
                },
            });
        }

        createOrEditCourseDialog.content.onSave.subscribe(() => {
            this.refresh();
        });
    }
}

