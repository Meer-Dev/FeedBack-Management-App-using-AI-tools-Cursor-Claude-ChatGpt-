import { Component, Injector, OnInit, EventEmitter, Output } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { CourseServiceProxy, CourseDto } from '@shared/service-proxies/service-proxies';
import { FormsModule } from '@angular/forms';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { CommonModule } from '@angular/common';
import { finalize } from 'rxjs/operators';

@Component({
    selector: 'app-create-course-dialog',
    templateUrl: './create-course-dialog.component.html',
    standalone: true,
    imports: [
        FormsModule,
        CommonModule,
        AbpValidationSummaryComponent, // Keep only what's used in template
        LocalizePipe,
    ],
})
export class CreateCourseDialogComponent extends AppComponentBase implements OnInit {
    @Output() onSave = new EventEmitter<any>();

    saving = false;
    course: CourseDto;

    constructor(
        injector: Injector,
        public _courseService: CourseServiceProxy,
        public bsModalRef: BsModalRef
    ) {
        super(injector);
        this.course = new CourseDto();
    }

    ngOnInit(): void {}

    save(): void {
        this.saving = true;

        this._courseService.create(this.course)
            .pipe(finalize(() => {
                this.saving = false;
            }))
            .subscribe({
                next: () => {
                    this.notify.info(this.l('SavedSuccessfully'));
                    this.bsModalRef.hide();
                    this.onSave.emit();
                },
                error: (error) => {
                    console.error('Error creating course:', error);
                    this.notify.error(this.l('SaveFailed'));
                }
            });
    }

    cancel(): void {
        this.bsModalRef.hide();
    }
}