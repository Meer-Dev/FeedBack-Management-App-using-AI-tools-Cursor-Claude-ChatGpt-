import { NgModule } from '@angular/core';
import { SharedModule } from '@shared/shared.module';
import { CreateFeedbackDialogComponent } from './create-feedback/create-feedback-dialog.component';
import { EditFeedbackDialogComponent } from './edit-feedback/edit-feedback-dialog.component';
import { FeedbacksRoutingModule } from './feedbacks-routing.module';
import { FeedbacksComponent } from './feedbacks.component';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';

@NgModule({
    imports: [
        SharedModule,
        FeedbacksRoutingModule,
        CommonModule,
        HttpClientModule,
        FeedbacksComponent,
        EditFeedbackDialogComponent,
        CreateFeedbackDialogComponent,
    ],
})
export class FeedbacksModule {}

