import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { PartnerPaged } from 'src/app/partners/partner-simple';
import { PartnerService } from 'src/app/partners/partner.service';
import { SmsManualDialogComponent } from '../sms-manual-dialog/sms-manual-dialog.component';
import { SmsTemplateCrUpComponent } from '../sms-template-cr-up/sms-template-cr-up.component';
import { SmsTemplateService } from '../sms-template.service';

@Component({
  selector: 'app-sms-birthday-form-manual',
  templateUrl: './sms-birthday-form-manual.component.html',
  styleUrls: ['./sms-birthday-form-manual.component.css']
})
export class SmsBirthdayFormManualComponent implements OnInit {

  // @ViewChild("smsTemplateCbx", { static: true }) smsTemplateCbx: ComboBoxComponent
  gridData: any;
  filteredSMSAccount: any[];
  filteredTemplate: any[];
  skip: number = 0;
  limit: number = 20;
  isRowSelected: any[];
  search: string = '';
  selectedIds: string[] = [];
  public today: Date = new Date(new Date().toDateString());

  constructor(
    private partnerService: PartnerService,
    private smsTemplateService: SmsTemplateService,
    private modalService: NgbModal,
  ) { }

  ngOnInit() {

    // this.loadSmsTemplate();

    // this.smsTemplateCbx.filterChange.asObservable().pipe(
    //   debounceTime(300),
    //   tap(() => (this.smsTemplateCbx.loading = true)),
    //   switchMap(value => this.searchSmsTemplate(value))
    // ).subscribe((result: any) => {
    //   this.filteredTemplate = result;
    //   this.smsTemplateCbx.loading = false;
    // });

    this.loadDataFromApi();
  }

  loadDataFromApi() {
    var val = new PartnerPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.customer = true;
    val.supplier = false;
    val.isBirthday = true;
    this.partnerService.getCustomerBirthDay(val)
      .subscribe((res: any[]) => {
        this.gridData = res;
      }, err => {
        console.log(err);
      }
      )
  }

  // loadSmsTemplate() {
  //   this.searchSmsTemplate().subscribe(
  //     (res: any) => {
  //       this.filteredTemplate = res;
  //     }
  //   )
  // }

  searchSmsTemplate(q?: string) {
    return this.smsTemplateService.getAutoComplete(q);
  }

  // addTemplate() {
  //   const modalRef = this.modalService.open(SmsTemplateCrUpComponent, { size: 'lg', windowClass: 'o_technical_modal' });
  //   modalRef.componentInstance.title = 'Tạo mẫu tin';
  //   modalRef.result.then((val) => {
  //     this.loadSmsTemplate();
  //   })
  // }

  pageChange(event) {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  onSend() {
    // if (this.formGroup.invalid) { return; }
    // var val = this.formGroup.value;
    // val.partnerIds = this.selectedIds ? this.selectedIds : [];
    // console.log(val);

    var modalRef = this.modalService.open(SmsManualDialogComponent, { size: "sm", windowClass: "o_technical_modal" });
    modalRef.componentInstance.title = "Tạo tin gửi";
    modalRef.componentInstance.id = this.selectedIds ? this.selectedIds : [];
    modalRef.result.then(
      result => {

      }
    )

    // val.configSMSId = val.configSMS.id;
    // this.composeMessageService.create(val).subscribe(
    //   result => {
    //     console.log(result);
    //   }
    // )
  }

}
