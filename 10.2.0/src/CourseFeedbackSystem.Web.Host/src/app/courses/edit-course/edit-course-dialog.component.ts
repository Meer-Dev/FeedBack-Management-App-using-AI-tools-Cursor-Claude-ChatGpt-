import { Component, Injector, OnInit, EventEmitter, Output, ChangeDetectorRef } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { CourseServiceProxy, CourseDto } from '@shared/service-proxies/service-proxies';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { LocalizePipe } from '@shared/pipes/localize.pipe';

@Component({
    selector: 'app-edit-course-dialog',  // ✅ ADD THIS
    templateUrl: './edit-course-dialog.component.html',
    standalone: true,
    imports: [
        CommonModule,  // ✅ ADD THIS
        FormsModule,
        AbpModalHeaderComponent,
        AbpValidationSummaryComponent,
        AbpModalFooterComponent,
        LocalizePipe,
    ],
})
export class EditCourseDialogComponent extends AppComponentBase implements OnInit {
    @Output() onSave = new EventEmitter<any>();

    saving = false;
    course: CourseDto = new CourseDto();
    id: number;

    constructor(
        injector: Injector,
        public _courseService: CourseServiceProxy,
        public bsModalRef: BsModalRef,
        private cd: ChangeDetectorRef
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this._courseService.get(this.id).subscribe((result: CourseDto) => {
            // ✅ Wrap in Promise.resolve() to avoid change detection error
            Promise.resolve().then(() => {
                this.course = result;
                this.cd.detectChanges();
            });
        });
    }

    save(): void {
        this.saving = true;

        this._courseService.update(this.course).subscribe(
            () => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.bsModalRef.hide();
                this.onSave.emit();
            },
            () => {
                this.saving = false;
                this.cd.detectChanges();
            }
        );
    }
}