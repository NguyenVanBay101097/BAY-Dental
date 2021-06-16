import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { SmsCampaignService } from '../sms-campaign.service';

@Component({
  selector: 'app-sms-campaign-cr-up',
  templateUrl: './sms-campaign-cr-up.component.html',
  styleUrls: ['./sms-campaign-cr-up.component.css']
})
export class SmsCampaignCrUpComponent implements OnInit {

  id: string;
  title: string;
  formGroup: FormGroup;
  submitted = false;

  get f() { return this.formGroup.controls; }

  constructor(
    private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private intlService: IntlService,
    private smsCampaignService: SmsCampaignService,
    private notificationService: NotificationService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      typeDate: "unlimited", // unlimited: vô thời hạn, period: khoảng thời gian
      startDateObj: [new Date(), Validators.required],
      endDateObj: [new Date(), Validators.required],
      limitMessage: [0, Validators.required]
    })
  }


  getValueFormControl(key) {
    return this.formGroup.get(key).value;
  }

  onSave() {
    this.submitted = true;
    if (this.formGroup.invalid) return false;
    var val = this.formGroup.value;
    val.dateEnd = this.intlService.formatDate(val.endDateObj, "yyyy-MM-ddT23:59");
    val.dateStart = this.intlService.formatDate(val.startDateObj, "yyyy-MM-dd");
    val.userCampaign = true;
    this.smsCampaignService.create(val).subscribe(
      () => {
        this.notify("Thêm chiến dịch thành công", true);
        this.activeModal.close();
      }
    )
  }

  notify(title, isSuccess = true) {
    this.notificationService.show({
      content: title,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: isSuccess ? 'success' : 'error', icon: true },
    });
  }
}
