import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-role-form-v2',
  templateUrl: './role-form-v2.component.html',
  styleUrls: ['./role-form-v2.component.css']
})
export class RoleFormV2Component implements OnInit {

  @Input() listFeature: any;
  permissionDict: any = {};

  constructor() { }

  ngOnInit() {
  }

  expand(item){
    item.expand = !item.expand;
  }


}
