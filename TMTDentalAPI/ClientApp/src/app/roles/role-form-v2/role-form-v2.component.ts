import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { element } from 'protractor';

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
