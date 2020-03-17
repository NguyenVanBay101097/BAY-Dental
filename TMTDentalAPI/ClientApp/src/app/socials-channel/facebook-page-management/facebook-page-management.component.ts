import { Component, OnInit } from '@angular/core';
import { FacebookPageService } from '../facebook-page.service';

@Component({
  selector: 'app-facebook-page-management',
  templateUrl: './facebook-page-management.component.html',
  styleUrls: ['./facebook-page-management.component.css']
})
export class FacebookPageManagementComponent implements OnInit {
  switchPage: any;
  constructor(private facebookPageService: FacebookPageService) { }

  ngOnInit() {
    this.loadSwitchPage();
  }

  loadSwitchPage() {
    this.facebookPageService.getSwitchPage().subscribe((result: any) => {
      console.log(result);
      this.switchPage = result;
    });
  }
}
