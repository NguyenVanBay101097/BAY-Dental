import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { PartnerPaged } from 'src/app/partners/partner-simple';
import { PartnerService } from 'src/app/partners/partner.service';

@Component({
  selector: 'app-sms-birthday-form-manual',
  templateUrl: './sms-birthday-form-manual.component.html',
  styleUrls: ['./sms-birthday-form-manual.component.css']
})
export class SmsBirthdayFormManualComponent implements OnInit {

  @ViewChild("configSMSCbx", { static: true }) configSMSCbx: ComboBoxComponent
  gridData: any;
  filteredConfigSMS: any[];
  formGroup: FormGroup;
  skip: number = 0;
  limit: number = 20;
  isRowSelected: any[];
  search: string = '';
  selectedIds: string[] = [];
  public today: Date = new Date(new Date().toDateString());
  
  constructor(
    private partnerService: PartnerService,
    // private smsService: SmsBrandNameService,
    // private composeMessageService: ComposeMessageService,
    private fb: FormBuilder
  ) { }

  ngOnInit() {
    this.loadConfigSMS();
    this.formGroup = this.fb.group({
      date: [new Date(), Validators.required],
      content: ['', Validators.required],
      configSMS: [null, Validators.required],
      state: 'draft',
    })
    // this.configSMSCbx.filterChange.asObservable().pipe(
    //   debounceTime(300),
    //   tap(() => (this.configSMSCbx.loading = true)),
    //   switchMap(value => this.searchConfigSMS(value))
    // ).subscribe(result => {
    //   this.filteredConfigSMS = result;
    //   this.configSMSCbx.loading = false;
    // });
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    // var val = new PartnerPaged();
    // val.limit = this.limit;
    // val.offset = this.skip;
    // val.search = this.search || '';
    // val.customer = true;
    // val.supplier = false;
    // val.isBirthday = true;
    // this.partnerService.getCustomerBirthDay(val)
    //   .subscribe((res: any[]) => {
    //     this.gridData = res;
    //   }, err => {
    //     console.log(err);
    //   }
    //   )
  }

  loadConfigSMS() {
    // this.searchConfigSMS().subscribe(
    //   (res: any) => {
    //     this.filteredConfigSMS = res;
    //     this.formGroup.get('configSMS').patchValue(res[0] ? res[0] : null)
    //   }
    // )
  }

  searchConfigSMS(q?: string) {
    var val = {
      limit: this.limit,
      offset: this.skip,
      search: q || ''
    }
    // return this.smsService.getAutoComplete(val);
  }

  pageChange(event) {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  onSend() {
    if (this.formGroup.invalid) { return; }
    var val = this.formGroup.value;
    val.partnerIds = this.selectedIds ? this.selectedIds : [];
    val.configSMSId = val.configSMS.id;
    // this.composeMessageService.create(val).subscribe(
    //   result => {
    //     console.log(result);
    //   }
    // )
  }

}
