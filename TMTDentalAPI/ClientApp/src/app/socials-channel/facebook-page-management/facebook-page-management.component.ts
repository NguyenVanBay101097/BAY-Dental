import { Component, OnInit } from '@angular/core';
import { FacebookPageService } from '../facebook-page.service';
import { ActivatedRoute, ParamMap } from '@angular/router';

@Component({
  selector: 'app-facebook-page-management',
  templateUrl: './facebook-page-management.component.html',
  styleUrls: ['./facebook-page-management.component.css']
})
export class FacebookPageManagementComponent implements OnInit {
  switchPage: any;
  page: any = {};
  id: string;
  constructor(private facebookPageService: FacebookPageService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.route.paramMap.subscribe((param: ParamMap) => {
      this.id = param.get('id');
      this.loadPage(this.id);
    });
  }

  loadPage(id) {
    this.facebookPageService.get(id).subscribe((result: any) => {
      this.page = result;
    });
  }

  loadSwitchPage() {
    this.facebookPageService.getSwitchPage().subscribe((result: any) => {
      console.log(result);
      this.switchPage = result;
    });
  }

  switchPageClick(page: any) {
    this.facebookPageService.selectPage(page.id).subscribe(() => {
      window.location.reload();
    });
  }
}
