import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AccountPaymentService, AccountPaymentDisplay } from '../account-payment.service';
import { FormGroup, FormBuilder } from '@angular/forms';
import { AppLoadingService } from 'src/app/shared/app-loading.service';

@Component({
  selector: 'app-account-payment-create-update',
  templateUrl: './account-payment-create-update.component.html',
  styleUrls: ['./account-payment-create-update.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class AccountPaymentCreateUpdateComponent implements OnInit {
  id: string;
  payment: AccountPaymentDisplay = new AccountPaymentDisplay();
  formGroup: FormGroup;
  constructor(private route: ActivatedRoute, private paymentService: AccountPaymentService,
    private fb: FormBuilder, private loadingService: AppLoadingService) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      paymentType: null
    });

    this.id = this.route.snapshot.params['id'];
    this.loadRecord();
  }

  loadRecord() {
    if (this.id) {
      this.paymentService.get(this.id).subscribe(result => {
        this.payment = result;
        this.formGroup.patchValue(result);
      });
    }
  }

  actionCancel() {
    if (this.id) {
      this.loadingService.setLoading(true);
      this.paymentService.actionCancel([this.id]).subscribe(() => {
        this.loadRecord();
        this.loadingService.setLoading(false);
      }, () => {
        this.loadingService.setLoading(false);
      });
    }
  }

  unlink() {
    if (this.id) {
      this.paymentService.unlink([this.id]).subscribe(() => {
        this.loadingService.setLoading(false);
        window.history.back();
      });
    }
  }
}
