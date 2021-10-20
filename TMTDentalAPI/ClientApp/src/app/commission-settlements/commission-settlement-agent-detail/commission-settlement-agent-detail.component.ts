import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AgentService } from 'src/app/agents/agent.service';
@Component({
  selector: 'app-commission-settlement-agent-detail',
  templateUrl: './commission-settlement-agent-detail.component.html',
  styleUrls: ['./commission-settlement-agent-detail.component.css']
})
export class CommissionSettlementAgentDetailComponent implements OnInit {
  agent: any;
  agentId: string;
  constructor(
    private route: ActivatedRoute,
    private agentService: AgentService
  ) { }

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.agentId = params.id;
      if (this.agentId) {
        this.loadAgent(this.agentId)
      }
    })
  }

  loadAgent(id: string) {
    this.agentService.get(id).subscribe(result => {
      this.agent = result;
    })
  }
}
