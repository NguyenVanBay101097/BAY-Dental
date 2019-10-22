import { Component, OnInit } from '@angular/core';
import { AppLoadingService } from '../app-loading.service';

@Component({
  selector: 'app-loading',
  templateUrl: './loading.component.html',
  styleUrls: ['./loading.component.css']
})
export class LoadingComponent implements OnInit {

  constructor(public loadingService: AppLoadingService) { }

  ngOnInit() {
  }

}
