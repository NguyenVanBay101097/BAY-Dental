import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-trial-registration-success',
  templateUrl: './trial-registration-success.component.html',
  styleUrls: ['./trial-registration-success.component.css']
})
export class TrialRegistrationSuccessComponent implements OnInit {
  data: any;
  constructor() { }

  ngOnInit() {
    console.log(history.state);
    this.data = history.state;
  }
}
