import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { PartnerSourceListComponent } from "./partner-source-list/partner-source-list.component";
import { PartnerSourceService } from "./partner-source.service";
import { DialogsModule } from "@progress/kendo-angular-dialog";
import { ButtonsModule } from "@progress/kendo-angular-buttons";

import { ReactiveFormsModule, FormsModule } from "@angular/forms";
import { SharedModule } from "../shared/shared.module";
import { GridModule } from "@progress/kendo-angular-grid";
import { MyCustomKendoModule } from "../shared/my-customer-kendo.module";
import { PartnerSourcesRoutingModule } from "./partner-sources-routing.module";
import { PartnerSourceCreateUpdateDialogComponent } from "./partner-source-create-update-dialog/partner-source-create-update-dialog.component";

@NgModule({
  declarations: [
    PartnerSourceListComponent,
    PartnerSourceCreateUpdateDialogComponent,
  ],
  imports: [
    PartnerSourcesRoutingModule,
    CommonModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    SharedModule,
    GridModule,
    ButtonsModule,
    DialogsModule,
    FormsModule,
  ],
  providers: [PartnerSourceService],
  entryComponents: [PartnerSourceCreateUpdateDialogComponent],
})
export class PartnerSourcesModule {}
