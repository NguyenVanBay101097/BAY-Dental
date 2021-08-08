import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CustomerReceiptReportForTimeComponent } from './customer-receipt-report-for-time/customer-receipt-report-for-time.component';
import { CustomerReceiptReportManageComponent } from './customer-receipt-report-manage/customer-receipt-report-manage.component';
import { CustomerReceiptReportNoTreatmentComponent } from './customer-receipt-report-no-treatment/customer-receipt-report-no-treatment.component';
import { CustomerReceiptReportOverviewComponent } from './customer-receipt-report-overview/customer-receipt-report-overview.component';
import { CustomerReceiptReportTimeserviceComponent } from './customer-receipt-report-timeservice/customer-receipt-report-timeservice.component';


const routes: Routes = [
    {
        path: '', component: CustomerReceiptReportManageComponent,
        children: [
            { path: 'customer-receipt-overview', component: CustomerReceiptReportOverviewComponent },
            { path: 'customer-receipt-for-time', component: CustomerReceiptReportForTimeComponent },
            { path: 'customer-receipt-timeservice', component: CustomerReceiptReportTimeserviceComponent },
            { path: 'customer-receipt-notreatment', component: CustomerReceiptReportNoTreatmentComponent },
            { path: '', redirectTo: 'customer-receipt-overview', pathMatch: 'full' }
        ]
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class CustomerReceiptReportRoutingModule { }