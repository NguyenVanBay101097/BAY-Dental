import { Component, OnInit } from '@angular/core';
import { FacebookPageService } from '../facebook-page.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-facebook-dashboard',
  templateUrl: './facebook-dashboard.component.html',
  styleUrls: ['./facebook-dashboard.component.css']
})
export class FacebookDashboardComponent implements OnInit {
  pages: any[] = [];
  constructor(private facebookPageService: FacebookPageService, private router: Router) { }

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

  selectPage(page: any) {
    this.facebookPageService.selectPage(page.id).subscribe(() => {
      this.router.navigate(['/facebook-management']);
    });
  }
}
