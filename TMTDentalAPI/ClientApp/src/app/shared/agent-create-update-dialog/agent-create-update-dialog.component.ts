import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import * as _ from 'lodash';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { AgentService } from 'src/app/agents/agent.service';
import { BankPaged, BankService } from 'src/app/bank/bank.service';
import { Commission, CommissionPaged, CommissionService } from 'src/app/commissions/commission.service';
import { PartnerPaged, PartnerSimple } from 'src/app/partners/partner-simple';
import { PartnerService } from 'src/app/partners/partner.service';
import { UserService } from 'src/app/users/user.service';
import { BankCuDialogComponent } from '../bank-cu-dialog/bank-cu-dialog.component';
import { CheckPermissionService } from '../check-permission.service';
import { NotifyService } from '../services/notify.service';

@Component({
  selector: 'app-agent-create-update-dialog',
  templateUrl: './agent-create-update-dialog.component.html',
  styleUrls: ['./agent-create-update-dialog.component.css']
})
export class AgentCreateUpdateDialogComponent implements OnInit, AfterViewInit {
  id: string;
  formGroup: FormGroup;
  submitted = false;
  isRoleRead = false;
  isRoleCreate = false;
  title: string;

  dayList: number[] = [];
  monthList: number[] = [];
  yearList: number[] = [];
  listAgentCommissions: Commission[] = [];
  listBank: any[] = [];
  customerSimpleFilter: PartnerSimple[] = [];
  @ViewChild('agentCommissionCbx', { static: false }) agentCommissionCbx: ComboBoxComponent;

  constructor(private fb: FormBuilder,
    public agentService: AgentService,
    public activeModal: NgbActiveModal,
    private modalService: NgbModal,
    private intlService: IntlService,
    private checkPermissionService: CheckPermissionService,
    private commissionService: CommissionService,
    private partnerService: PartnerService,
    private notifyService: NotifyService,
    private bankService: BankService
  ) { }


  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ["", Validators.required],
      gender: "male",
      birthDayStr: '',
      birthMonthStr: '',
      birthYearStr: '',
      email: null,
      phone: null,
      jobTitle: null,
      address: null,
      commission: [null, Validators.required],
    });

    setTimeout(() => {
      this.checkRole();
      this.reload();
      this.dayList = _.range(1, 32);
      this.monthList = _.range(1, 13);
      this.yearList = _.range(new Date().getFullYear(), 1900, -1);
      this.loadListcommissionAgents();
      this.loadBank();
    })
  }

  ngAfterViewInit(): void {
    this.agentCommissionCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.agentCommissionCbx.loading = true)),
      switchMap(value => this.searchCommissionAgents(value))
    ).subscribe(result => {
      this.listAgentCommissions = result.items;
      this.agentCommissionCbx.loading = false;
    });
  }

  // searchCustomers(search) {
  //   var partnerPaged = new PartnerPaged();
  //   partnerPaged.customer = true;
  //   if (search) {
  //     partnerPaged.search = search.toLowerCase();
  //   }
  //   return this.partnerService.autocompletePartner(partnerPaged);
  // }

  loadBank() {
    let val = new BankPaged();
    val.limit = 20;
    val.offset = 0;
    val.search = '';
    this.bankService.getAutocomplete(val).subscribe((res: any) => {
      console.log(res);

    })
  }

  loadListcommissionAgents() {
    this.searchCommissionAgents().subscribe(result => {
      this.listAgentCommissions = _.unionBy(this.listAgentCommissions, result.items, 'id');
    });
  }

  searchCommissionAgents(q?: string) {
    var val = new CommissionPaged();
    val.search = q || '';
    val.type = 'agent';
    return this.commissionService.getPaged(val);
  }

  reload() {
    if (this.id) {
      this.agentService.get(this.id).subscribe((result) => {
        this.formGroup.patchValue(result);
        if (result.birthYear) {
          this.formGroup.get("birthYearStr").setValue(result.birthYear + '');
        }

        if (result.birthMonth) {
          this.formGroup.get("birthMonthStr").setValue(result.birthMonth + '');
        }

        if (result.birthDay) {
          this.formGroup.get("birthDayStr").setValue(result.birthDay + '');
        }

      });
    }
  }

  get f() { return this.formGroup.controls; }

  onSave() {
    this.submitted = true;

    if (!this.formGroup.valid) {
      return false;
    }

    var val = this.formGroup.value;
    val.birthDay = val.birthDayStr ? parseInt(val.birthDayStr) : null;
    val.birthMonth = val.birthMonthStr ? parseInt(val.birthMonthStr) : null;
    val.birthYear = val.birthYearStr ? parseInt(val.birthYearStr) : null;
    val.commissionId = val.commission ? val.commission.id : null;
    if (this.id) {
      this.agentService.update(this.id, val).subscribe(
        () => {
          this.activeModal.close(true);
        },
      );
    } else {
      this.agentService.create(val).subscribe(
        (result) => {
          this.activeModal.close(result);
        },
      );
    }
  }

  onCancel() {
    this.activeModal.dismiss();
  }

  checkRole() {
    this.isRoleRead = this.checkPermissionService.check(["Catalog.Agent.Read"]);
    this.isRoleCreate = this.checkPermissionService.check(["Catalog.Agent.Create"]);
  }

  createBank() {
    const modalRef = this.modalService.open(BankCuDialogComponent, { scrollable: true, windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm ngân hàng';

    modalRef.result.then(result => {
      this.notifyService.notify('success', 'Lưu thành công');

    })
  }
}
