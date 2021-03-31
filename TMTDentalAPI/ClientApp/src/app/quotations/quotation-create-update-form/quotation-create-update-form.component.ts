import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { IntlService } from '@progress/kendo-angular-intl';
import { switchMap } from 'rxjs/operators';
import { QuotationService } from '../quotation.service';

@Component({
  selector: 'app-quotation-create-update-form',
  templateUrl: './quotation-create-update-form.component.html',
  styleUrls: ['./quotation-create-update-form.component.css']
})
export class QuotationCreateUpdateFormComponent implements OnInit {
  formGroup: FormGroup;
  partner: any;
  partnerId: string;
  quotationId: string;
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());

  constructor(
    private fb: FormBuilder,
    private activatedRoute: ActivatedRoute,
    private quotationService: QuotationService,
    private intlService: IntlService,

  ) { }

  ngOnInit() {
    // debugger;
    this.formGroup = this.fb.group({
      partnerId: [null, Validators.required],
      userId: [null, Validators.required],
      note: '',
      dateQuotation: [null, Validators.required],
      dateApplies: [null, Validators.required],
      dateEndQuotation: ''
    })
    this.formGroup.get('dateQuotation').patchValue(this.monthStart);
    this.formGroup.get('dateApplies').patchValue(30);
    
    this.formGroup.get('dateEndQuotation').patchValue(this.getDate(this.monthStart));
    
    // this.routeActive();


  }

  routeActive() {
    this.activatedRoute.params.pipe(
      switchMap((params: ParamMap) => {
        this.quotationId = params.get("id");
        this.partnerId = params.get("partner_id");
        if (this.quotationId) {
          return this.quotationService.get(this.quotationId);
        } else {
          return this.quotationService.defaultGet(this.partnerId);
        }
      })).subscribe(
        result => {
          this.partner = result;
          this.formGroup.patchValue(result);

        }
      )
  }

  loadDataFromApi() {

  }

  loadDefault() {

  }

  addLine(val) {
    // this.saleOrderLine = event;
  }

  onSave() {

  }

  getDate(val) {
    let dateAppliesChange = this.formGroup.get('dateApplies').value;
    let dateChange = new Date(val);
    dateChange.setDate(dateChange.getDate() + dateAppliesChange);
    let dateEndQuotationChange = this.intlService.formatDate(dateChange, 'dd-MM-yyyy');
    return dateEndQuotationChange;
  }

  onDateChange(event) {
    this.formGroup.get('dateEndQuotation').patchValue(this.getDate(event));
  }
}


