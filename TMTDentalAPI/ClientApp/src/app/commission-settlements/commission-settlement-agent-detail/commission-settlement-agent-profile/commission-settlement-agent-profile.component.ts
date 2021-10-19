import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AgentService } from 'src/app/agents/agent.service';
import { AgentCreateUpdateDialogComponent } from 'src/app/shared/agent-create-update-dialog/agent-create-update-dialog.component';
import { NotifyService } from 'src/app/shared/services/notify.service';

@Component({
  selector: 'app-commission-settlement-agent-profile',
  templateUrl: './commission-settlement-agent-profile.component.html',
  styleUrls: ['./commission-settlement-agent-profile.component.css']
})
export class CommissionSettlementAgentProfileComponent implements OnInit {
  agentId: string;
  agentObj: any;
  constructor(
    private route: ActivatedRoute,
    private agentService: AgentService,
    private modalService: NgbModal,
    private notifyService: NotifyService,
  ) { }

  ngOnInit() {
    this.agentId = this.route.parent.snapshot.paramMap.get('id');
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    if (this.agentId) {
      this.agentService.get(this.agentId).subscribe((result) => {
        this.agentObj = result
        console.log(result);
      })
    }
  }

  editAgent(){
    const modalRef = this.modalService.open(AgentCreateUpdateDialogComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa người giới thiệu';
    modalRef.componentInstance.id = this.agentId;
    modalRef.result.then(() => {
      this.notifyService.notify('success','Lưu thành công');
      this.loadDataFromApi();
    }, () => {
    })
  }
}
