import { NgModule } from '@angular/core';
import { SharedModule } from '@shared/shared.module';
import { CreateCourseDialogComponent } from './create-course/create-course-dialog.component';
import { EditCourseDialogComponent } from './edit-course/edit-course-dialog.component';
import { CoursesRoutingModule } from './courses-routing.module';
import { CoursesComponent } from './courses.component';
import { CommonModule } from '@angular/common';

@NgModule({
    imports: [
        SharedModule,
        CoursesRoutingModule,
        CommonModule,
        CoursesComponent,
        EditCourseDialogComponent,
        CreateCourseDialogComponent, // Make sure this component exists and is exported
    ],
})
export class CoursesModule {}