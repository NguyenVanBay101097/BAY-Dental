import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { SaleSettingsService } from '../sale-settings.service';
import { NotificationService } from '@progress/kendo-angular-notification';

@Component({
  selector: 'app-sale-settings-overview',
  templateUrl: './sale-settings-overview.component.html',
  styleUrls: ['./sale-settings-overview.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class SaleSettingsOverviewComponent implements OnInit {
  formGroup: FormGroup;
  constructor(private fb: FormBuilder, private saleSettingsService: SaleSettingsService,
    private notificationService: NotificationService) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      pointExchangeRate: null,
    });

    this.loadSetting();
  }

  loadSetting() {
    this.saleSettingsService.getSetting().subscribe(result => {
      this.formGroup.patchValue(result);
    });
  }

  onSave() {
    if (!this.formGroup.valid) {
      return false;
    }

    var val = this.formGroup.value;
    this.saleSettingsService.update(val).subscribe(() => {
      this.notificationService.show({
        content: 'Lưu thành công',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });
    });
  }
}
