import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import * as _ from 'lodash';
import { debounceTime, mergeMap, switchMap, tap } from 'rxjs/operators';
import { AccountJournalFilter, AccountJournalService } from 'src/app/account-journals/account-journal.service';
import { AgentService, TotalAmountAgentFilter } from 'src/app/agents/agent.service';
import { AuthService } from 'src/app/auth/auth.service';
import { PhieuThuChiService } from 'src/app/phieu-thu-chi/phieu-thu-chi.service';

@Component({
  selector: 'app-commission-settlement-agent-payment-dialog',
  templateUrl: './commission-settlement-agent-payment-dialog.component.html',
  styleUrls: ['./commission-settlement-agent-payment-dialog.component.css']
})
export class CommissionSettlementAgentPaymentDialogComponent implements OnInit {
  title: string;
  type: string;
  agentId: string;
  partnerId: string;
  accountType: string;
  formGroup: FormGroup;
  submitted: boolean = false;
  amountBalanceTotal : number;
  resAgent: any;
  filteredJournals: any = [];

  @ViewChild("journalCbx", { static: true }) journalCbx: ComboBoxComponent;

  constructor(
    private fb: FormBuilder,
    private intlService: IntlService,
    public activeModal: NgbActiveModal,
    private authService: AuthService,
    private phieuthuchiService: PhieuThuChiService,
    private agentService: AgentService,
    private accountJournalService: AccountJournalService,
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      amount: [0, Validators.required],
      dateObj: [null, Validators.required],
      journal: [null, Validators.required],
      reason: null,
    });

    this.loadDefault();
    this.loadFilteredJournals();

    this.journalCbx.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.journalCbx.loading = true)),
        switchMap((value) => this.searchFilteredJournals(value)
        )
      )
      .subscribe((result: any) => {
        this.filteredJournals = result;
        this.journalCbx.loading = false;
    });
  }

  loadDefault() {

    if(this.resAgent){
      this.formGroup.patchValue(this.resAgent);
      var paymentDate = new Date(this.resAgent.date);
      this.formGroup.get('dateObj').setValue(paymentDate);

      if (this.resAgent) {
        this.filteredJournals = _.unionBy(this.filteredJournals, this.resAgent.journal, 'id');
      }

    }else{
      this.phieuthuchiService.defaultGet({ type: this.type }).subscribe((rs: any) => {
        this.formGroup.patchValue(rs);
        var paymentDate = new Date(rs.date);
        this.formGroup.get('dateObj').setValue(paymentDate);
  
        if (rs.journal) {
          this.filteredJournals = _.unionBy(this.filteredJournals, rs.journal, 'id');
        }
  
      })
    }

  }


  loadFilteredJournals() {
    this.searchFilteredJournals().subscribe((res) => {
      this.filteredJournals = _.unionBy(this.filteredJournals, res, 'id');
    },
      (error) => {
        console.log(error);
      }
    );
  }

  searchFilteredJournals(q?: string) {
    var val = new AccountJournalFilter();
    val.type = "bank,cash";
    val.search = q || '';
    val.companyId = this.authService.userInfo.companyId;
    return this.accountJournalService.autocomplete(val);
  }


  get f() { return this.formGroup.controls; }

  actionPayment() {
    this.submitted = true;

    if (!this.formGroup.valid) {
      return false;
    }

    var val = this.formGroup.value;
    val.journalId = val.journal ? val.journal.id : null;
    val.agentId = this.agentId ? this.agentId : null;
    val.partnerId = this.partnerId ? this.partnerId : null;
    val.accountType = this.accountType;
    val.type = this.type;
    val.date = this.intlService.formatDate(val.dateObj, "yyyy-MM-ddTHH:mm:ss");
    this.phieuthuchiService.create(val).pipe(mergeMap((res: any) => {
      return this.phieuthuchiService.actionConfirm([res.id]);
    }))
      .subscribe(r => {
        this.activeModal.close(true);
      });
  }

  onClose() {
    this.activeModal.dismiss();
  }

}
