import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { IntlService } from '@progress/kendo-angular-intl';
import { TenantService } from '../tenant.service';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-tenant-update-info-dialog',
  templateUrl: './tenant-update-info-dialog.component.html',
  styleUrls: ['./tenant-update-info-dialog.component.css']
})

export class TenantUpdateInfoDialogComponent implements OnInit {
  title = 'Cập nhật thông tin';
  formGroup: FormGroup;
  id: string;

  constructor(private fb: FormBuilder, private intlService: IntlService, private tenantService: TenantService,
    public activeModal: NgbActiveModal) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      email: [null, [Validators.required, Validators.email]],
      phone: ['', [Validators.required, Validators.pattern('^[0-9]{10}$')]],
      customerSource: null,
      supporterName: null,
      companyName: ['', Validators.required],
      address: null
    });

    this.loadRecord();
  }

  get email() { return this.formGroup.get('email'); }

  get phone() { return this.formGroup.get('phone'); }

  loadRecord() {
    this.tenantService.get(this.id).subscribe((result: any) => {
      this.formGroup.patchValue(result);
    });
  }

  onSave() {
    var val = this.formGroup.value;
    this.tenantService.updateInfo(this.id, val).subscribe(() => {
      this.activeModal.close(true);
    }, (err) => {
      if (err.message) {
        alert(err.message);
      }
    });
  }
}
