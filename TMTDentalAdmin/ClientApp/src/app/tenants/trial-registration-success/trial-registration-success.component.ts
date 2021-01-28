import { Component, OnInit } from '@angular/core';
import { environment } from '@env';

@Component({
  selector: 'app-trial-registration-success',
  templateUrl: './trial-registration-success.component.html',
  styleUrls: ['./trial-registration-success.component.css']
})
export class TrialRegistrationSuccessComponent implements OnInit {
  data: any;
  constructor() { }

  ngOnInit() {
    this.data = history.state;
  }

  getHostName() {
    return environment.catalogScheme + '://' + this.data.hostName + '.' + environment.catalogHost;
  }
}
