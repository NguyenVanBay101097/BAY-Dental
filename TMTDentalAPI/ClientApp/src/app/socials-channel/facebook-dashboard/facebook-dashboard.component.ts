import { Component, OnInit } from '@angular/core';
import { FacebookPageService } from '../facebook-page.service';

@Component({
  selector: 'app-facebook-dashboard',
  templateUrl: './facebook-dashboard.component.html',
  styleUrls: ['./facebook-dashboard.component.css']
})
export class FacebookDashboardComponent implements OnInit {
  pages: any[] = [];
  constructor(private facebookPageService: FacebookPageService) { }

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    var val = {
      limit: 100
    };

    this.facebookPageService.getPaged(val).subscribe((result: any) => {
      this.pages = result.items;
    });
  }
}
