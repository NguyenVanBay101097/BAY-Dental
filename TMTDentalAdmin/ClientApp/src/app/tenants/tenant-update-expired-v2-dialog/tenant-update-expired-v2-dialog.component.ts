import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { IntlService } from '@progress/kendo-angular-intl';
import { TenantService } from '../tenant.service';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-tenant-update-expired-v2-dialog',
  templateUrl: './tenant-update-expired-v2-dialog.component.html',
  styleUrls: ['./tenant-update-expired-v2-dialog.component.css']
})
export class TenantUpdateExpiredV2DialogComponent implements OnInit {
  title = 'Gia hạn';
  formGroup: FormGroup;
  item: any;
  constructor(private fb: FormBuilder, private intlService: IntlService, private tenantService: TenantService,
    public activeModal: NgbActiveModal) { }

  ngOnInit() {
    console.log(this.item);
    this.formGroup = this.fb.group({
      dateExpired: this.item.dateExpired ? new Date(this.item.dateExpired) : null,
      activeCompaniesNbr: this.item.activeCompaniesNbr ? this.item.activeCompaniesNbr : 1
    });
  }

  onSave() {
    var val = this.formGroup.value;
    val.dateExpired = this.intlService.formatDate(val.dateExpired, 'yyyy-MM-ddTHH:mm:ss');
    val.id = this.item.id;
    console.log(val);
    this.tenantService.updateDateExpired(val).subscribe(() => {
      this.activeModal.close(true);
    });
  }
}
