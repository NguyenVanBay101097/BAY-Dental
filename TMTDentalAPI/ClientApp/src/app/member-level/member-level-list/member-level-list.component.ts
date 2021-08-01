import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MemberLevelService } from '../member-level.service';

@Component({
  selector: 'app-member-level-list',
  templateUrl: './member-level-list.component.html',
  styleUrls: ['./member-level-list.component.css']
})
export class MemberLevelListComponent implements OnInit {
  memberLevels: any[] = [];
  memberColors = [
    { id: '0' },
    { id: '1' },
    { id: '2' },
    { id: '3' },
    { id: '4' },
    { id: '5' },
    { id: '6' },
    { id: '7' },
    { id: '8' },
    { id: '9' },
    { id: '10' },
    { id: '11' },
  ]

  constructor(
    private router: Router,
    private memberLevelService: MemberLevelService
  ) { }

  ngOnInit() {
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.memberLevelService.get().subscribe((result) => {
      this.memberLevels = result;
    })
  }
  
  onUpdate(){
    this.router.navigate(['member-level/create']);
  }

}
