import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NotificationService } from '@progress/kendo-angular-notification';
import * as _ from 'lodash';
import { TCareConfigService } from '../tcare-config.service';

@Component({
  selector: 'app-tcare-config',
  templateUrl: './tcare-config.component.html',
  styleUrls: ['./tcare-config.component.css']
})
export class TcareConfigComponent implements OnInit {

  constructor(
    private configService: TCareConfigService,
    private fb: FormBuilder,
    private notificationService: NotificationService
  ) { }
  hourList: number[] = [];
  minuteList: number[] = [];
  formGroup: FormGroup;

  ngOnInit() {
    this.hourList = _.range(0, 24);
    this.minuteList = _.range(0, 60);

    this.formGroup = this.fb.group({
      id: [null, Validators.required],
      jobCampaignHour: [null, Validators.required],
      jobCampaignMinute: [null, Validators.required],
      jobMessagingMinute: [null, Validators.required],
      jobMessageMinute: [null, Validators.required],
    });
    this.loadData();
  }

  loadData() {
    this.configService.getFirst().subscribe((res: any) => {
      this.formGroup.patchValue(res);
    });
  }

  onSave() {
    if (this.formGroup.invalid) {
      return;
    }
    const val = this.formGroup.value;
    this.configService.update(val.id, val).subscribe(() => {
      this.loadData();
      this.notificationService.show({
        content: 'thành công!',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true },
      });
    });
  }
}
