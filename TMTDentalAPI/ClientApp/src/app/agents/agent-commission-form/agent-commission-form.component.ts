import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subject } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { AgentService } from '../agent.service';

@Component({
  selector: 'app-agent-commission-form',
  templateUrl: './agent-commission-form.component.html',
  styleUrls: ['./agent-commission-form.component.css']
})
export class AgentCommissionFormComponent implements OnInit {
  id: string;
  agent: any;
  amountInComeAgent = 0;
  amountCommissionAgent = 0;
  @Input() updateSubject: Subject<boolean> = new Subject<boolean>();
  constructor(private agentService: AgentService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.id = this.route.snapshot.paramMap.get('id');
    this.loadDataFromApi();
    this.loadAmountInComeAgent();
    this.loadAmountCommissionAgent();

    this.updateSubject.subscribe(response => {
      
      if (response) {
        this.reload();
      }
    });
  }

  loadDataFromApi() {
    if (this.id) {
      this.agentService.get(this.id).subscribe((result) => {
        this.agent = result;
      });
    }
  }

  loadAmountCommissionAgent() {
    if (this.id) {
      this.agentService.getAmountCommissionAgent(this.id).subscribe((result) => {
        this.amountCommissionAgent = result;
      });
    }
  }

  loadAmountInComeAgent() {
    if (this.id) {
      this.agentService.getAmountInComeAgent(this.id).subscribe((result) => {
        this.amountInComeAgent = result;
      });
    }
  }

  reload() {
      this.loadAmountCommissionAgent();
      this.loadAmountInComeAgent();
  }

}
