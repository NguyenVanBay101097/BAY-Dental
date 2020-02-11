import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { IntlService } from '@progress/kendo-angular-intl';
import { TenantService } from '../tenant.service';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-tenant-update-expired-dialog',
  templateUrl: './tenant-update-expired-dialog.component.html',
  styleUrls: ['./tenant-update-expired-dialog.component.css']
})
export class TenantUpdateExpiredDialogComponent implements OnInit {
  title = 'Gia hạn';
  formGroup: FormGroup;
  id: string;
  constructor(private fb: FormBuilder, private intlService: IntlService, private tenantService: TenantService,
    public activeModal: NgbActiveModal) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      dateExpired: null
    });
  }

  onSave() {
    var val = this.formGroup.value;
    val.dateExpired = this.intlService.formatDate(val.dateExpired, 'yyyy-MM-ddTHH:mm:ss');
    val.id = this.id;
    this.tenantService.updateDateExpired(val).subscribe(() => {
      this.activeModal.close(true);
    });
  }
}
