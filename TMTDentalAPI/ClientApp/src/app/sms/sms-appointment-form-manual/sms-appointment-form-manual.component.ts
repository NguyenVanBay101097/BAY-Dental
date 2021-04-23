import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { PartnerPaged } from 'src/app/partners/partner-simple';
import { PartnerService } from 'src/app/partners/partner.service';
import { SmsTemplateCrUpComponent } from '../sms-template-cr-up/sms-template-cr-up.component';
import { SmsTemplateService } from '../sms-template.service';

@Component({
  selector: 'app-sms-appointment-form-manual',
  templateUrl: './sms-appointment-form-manual.component.html',
  styleUrls: ['./sms-appointment-form-manual.component.css']
})
export class SmsAppointmentFormManualComponent implements OnInit {
  @ViewChild("smsTemplateCbx", { static: true }) smsTemplateCbx: ComboBoxComponent

  formGroup: FormGroup;
  filteredTemplate: any[];
  gridData: any;
  skip: number = 0;
  limit: number = 20;
  isRowSelected: any[];
  search: string = '';
  selectedIds: string[] = [];

  constructor(
    private modalService: NgbModal,
    private smsTemplateService: SmsTemplateService,
    private partnerService: PartnerService,
    private fb: FormBuilder

  ) { }

  ngOnInit() {
    this.loadDataFromApi();
    this.loadSmsTemplate();
    this.formGroup = this.fb.group({
      date: [new Date(), Validators.required],
      content: [null, Validators.required],
      state: 'draft',
    })

    this.smsTemplateCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.smsTemplateCbx.loading = true)),
      switchMap(value => this.searchSmsTemplate(value))
    ).subscribe((result: any) => {
      this.filteredTemplate = result;
      this.smsTemplateCbx.loading = false;
    });
  }

  addTemplate() {
    const modalRef = this.modalService.open(SmsTemplateCrUpComponent, { size: 'lg', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Tạo mẫu tin';
    modalRef.result.then((val) => {
      this.loadSmsTemplate();
    })
  }

  loadDataFromApi() {
    var val = new PartnerPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.customer = true;
    val.supplier = false;
    this.partnerService.getCustomerAppointments(val)
      .subscribe((res: any[]) => {
        this.gridData = res;
      }, err => {
        console.log(err);
      }
      )
  }

  loadSmsTemplate() {
    this.searchSmsTemplate().subscribe(
      (res: any) => {
        this.filteredTemplate = res;
        this.formGroup.get('content').patchValue(res[0] ? res[0] : null)
      }
    )
  }

  searchSmsTemplate(q?: string) {
    return this.smsTemplateService.getAutoComplete(q);
  }
  pageChange(event) {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  onSend() {
    if (this.formGroup.invalid) { return; }
    var val = this.formGroup.value;
    val.partnerIds = this.selectedIds ? this.selectedIds : [];
    console.log(val);
  }
}
