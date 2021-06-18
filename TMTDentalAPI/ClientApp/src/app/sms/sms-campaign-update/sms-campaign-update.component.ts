import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Validators } from '@angular/forms';
import { FormBuilder, FormGroup } from '@angular/forms';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { SmsCampaignService } from '../sms-campaign.service';

@Component({
  selector: 'app-sms-campaign-update',
  templateUrl: './sms-campaign-update.component.html',
  styleUrls: ['./sms-campaign-update.component.css']
})
export class SmsCampaignUpdateComponent implements OnInit {
  @Input() campaign: any;
  isEditting = false;
  formGroup: FormGroup;
  submitted = false;
  @Output() saveEvent = new EventEmitter<any>();

  constructor(private fb: FormBuilder,
    private intlService: IntlService,
    private smsCampaignService: SmsCampaignService,
    private notificationService: NotificationService) { }

  ngOnInit() {
    this.formGroup = this.fb.group({});
  }

  get f() {
    return this.formGroup.controls;
  }

  getValueFormControl(key) {
    return this.formGroup.get(key).value;
  }

  onEdit() {
    this.isEditting = true;
    this.formGroup = this.fb.group({
      name: [this.campaign.name, Validators.required],
      typeDate: [this.campaign.typeDate, Validators.required],
      dateStartObj: [this.campaign.dateStart ? new Date(this.campaign.dateStart) : null],
      dateEndObj: [this.campaign.dateEnd ? new Date(this.campaign.dateEnd) : null],
      limitMessage: [this.campaign.limitMessage, Validators.required]
    });
  }

  onSave() {
    this.submitted = true;
    if (this.formGroup.invalid) {
      return false;
    }
    
    var val = this.formGroup.value;
    val.dateEnd = val.dateEndObj ? this.intlService.formatDate(val.dateEndObj, "yyyy-MM-dd") : null;
    val.dateStart = val.dateStartObj ? this.intlService.formatDate(val.dateStartObj, "yyyy-MM-dd") : null;
    this.smsCampaignService.update(this.campaign.id, val).subscribe(
      () => {
        this.notify('Lưu thành công', 'success');
        this.isEditting = false;
        this.saveEvent.emit(true);
      }
    )
  }

  onCancel() {
    this.isEditting = false;
    this.formGroup = this.fb.group({});
  }

  notify(title, style) {
    this.notificationService.show({
      content: title,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: style, icon: true },
    });
  }
}
