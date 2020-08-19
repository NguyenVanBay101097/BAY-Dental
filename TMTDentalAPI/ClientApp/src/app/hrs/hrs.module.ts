import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { HrsRoutingModule } from './hrs-routing.module';
import { PayrollStructureListComponent } from './PayrollStructures/payroll-structure-list/payroll-structure-list.component';
import { PayrollStructureCreateUpdateComponent } from './PayrollStructures/payroll-structure-create-update/payroll-structure-create-update.component';
import { PayrollStructureService } from './PayrollStructures/PayrollStructure.service';
import { SharedModule, ColumnResizingService } from '@progress/kendo-angular-grid';
import { SalaryRuleListComponent } from './PayrollStructures/salary-rule-list/salary-rule-list.component';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { SalaryRuleCrudDialogComponent } from './PayrollStructures/salary-rule-crud-dialog/salary-rule-crud-dialog.component';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { HrPayrollStructureTypeListComponent } from './hr-payroll-structure-type-list/hr-payroll-structure-type-list.component';
import { HrPayrollStructureTypeCreateComponent } from './hr-payroll-structure-type-create/hr-payroll-structure-type-create.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';

@NgModule({
  declarations: [HrPayrollStructureTypeListComponent, HrPayrollStructureTypeCreateComponent, PayrollStructureListComponent, PayrollStructureCreateUpdateComponent, SalaryRuleListComponent, SalaryRuleCrudDialogComponent],
  imports: [
    CommonModule,
    HrsRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    FormsModule,
    SharedModule,
    LayoutModule,
    DropDownsModule,
    NgbModule
  ],
  providers: [
    PayrollStructureService,
    ColumnResizingService
  ],
  entryComponents: [
    SalaryRuleCrudDialogComponent, HrPayrollStructureTypeCreateComponent
  ]
})
export class HrsModule { }
