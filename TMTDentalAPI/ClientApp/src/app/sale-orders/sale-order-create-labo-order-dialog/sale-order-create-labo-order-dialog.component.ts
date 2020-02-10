import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { DotKhamService } from 'src/app/dot-khams/dot-kham.service';
import { IntlService } from '@progress/kendo-angular-intl';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import { EmployeePaged, EmployeeSimple } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { DotKhamDefaultGet } from 'src/app/dot-khams/dot-khams';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { EmployeeCreateUpdateComponent } from 'src/app/employees/employee-create-update/employee-create-update.component';
import * as _ from 'lodash';
import { PartnerSimple, PartnerPaged } from 'src/app/partners/partner-simple';
import { PartnerService } from 'src/app/partners/partner.service';

@Component({
  selector: 'app-sale-order-create-labo-order-dialog',
  templateUrl: './sale-order-create-labo-order-dialog.component.html',
  styleUrls: ['./sale-order-create-labo-order-dialog.component.css']
})

export class SaleOrderCreateLaboOrderDialogComponent implements OnInit {
  formGroup: FormGroup;
  saleOrderId: string;
  filteredPartners: PartnerSimple[];
  @ViewChild('partnerCbx', { static: true }) partnerCbx: ComboBoxComponent;
  title: string;

  constructor(private fb: FormBuilder, private partnerService: PartnerService, private intlService: IntlService,
    private employeeService: EmployeeService, public activeModal: NgbActiveModal, private modalService: NgbModal) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      partner: [null, Validators.required],
    });

    setTimeout(() => {
      this.partnerCbx.focus();
    }, 200);


    setTimeout(() => {
      this.loadFilteredPartners();

      this.partnerCbx.filterChange.asObservable().pipe(
        debounceTime(300),
        tap(() => (this.partnerCbx.loading = true)),
        switchMap(value => this.searchPartners(value))
      ).subscribe(result => {
        this.filteredPartners = result;
        this.partnerCbx.loading = false;
      });
    });
  }

  searchPartners(filter?: string) {
    var val = new PartnerPaged();
    val.supplier = true;
    val.searchNamePhoneRef = filter;
    return this.partnerService.getAutocompleteSimple(val);
  }

  onSave() {
    if (!this.formGroup.valid) {
      return;
    }

    var val = this.formGroup.value;
    val.doctorId = val.doctor ? val.doctor.id : null;
    val.assistantId = val.assistant ? val.assistant.id : null;
  }

  onSaveAndView() {
    if (!this.formGroup.valid) {
      return;
    }

    var val = this.formGroup.value;
    val.doctorId = val.doctor ? val.doctor.id : null;
    val.assistantId = val.assistant ? val.assistant.id : null;
  }

  onCancel() {
    this.activeModal.dismiss();
  }

  loadFilteredPartners() {
    this.searchPartners().subscribe(
      rs => {
        this.filteredPartners = _.unionBy(this.filteredPartners, rs, 'id');
      });
  }
}


