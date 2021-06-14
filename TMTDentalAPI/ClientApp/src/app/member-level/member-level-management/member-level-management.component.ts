import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MemberLevelService } from '../member-level.service';

@Component({
  selector: 'app-member-level-management',
  templateUrl: './member-level-management.component.html',
  styleUrls: ['./member-level-management.component.css']
})
export class MemberLevelManagementComponent implements OnInit {
  constructor(
    private route: ActivatedRoute,
    private router: Router,
  ) { }

  ngOnInit() {
    this.route.data.subscribe((data: { memberLevelsResolve: any }) => {
      if (data.memberLevelsResolve.length > 0) {
        this.router.navigate(['member-level/list']);
      }
    })
  }

  createLevel() {
    this.router.navigate(['member-level/create']);
  }

}
