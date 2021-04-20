import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-role-form-v2',
  templateUrl: './role-form-v2.component.html',
  styleUrls: ['./role-form-v2.component.css']
})
export class RoleFormV2Component implements OnInit {

  @Input () listRoles: any;
  arr = [
    {
      id: 1,
      name: "ABC",
      groupName: "Hệ thống",
      checked: false,
      expand: false,
      amount_checked: 2,
      child: [
        {
          id: 1.1,
          name: "See List",
          checked: true
        },
        {
          id: 1.2,
          name: "Edit",
          checked: false
        },
        {
          id: 1.3,
          name: "Delete",
          checked: true
        },
        {
          id: 1.4,
          name: "Print",
          checked: false
        }
      ]
    },
    {
      id: 2,
      name: "DEF",
      groupName: "Hàng hóa",
      checked: false,
      expand: false,
      amount_checked: 1,
      child: [
        {
          id: 2.1,
          name: "See List",
          checked: false
        },
        {
          id: 2.2,
          name: "Edit",
          checked: false
        },
        {
          id: 2.3,
          name: "Delete",
          checked: true
        },
        {
          id: 2.4,
          name: "Print",
          checked: false
        }
      ]
    },
    {
      id: 3,
      name: "GHI",
      groupName: "Báo cáo",
      checked: false,
      expand: false,
      amount_checked: 0,
      child: [
        {
          id: 3.1,
          name: "See List",
          checked: false
        },
        {
          id: 3.2,
          name: "Edit",
          checked: false
        },
        {
          id: 3.3,
          name: "Delete",
          checked: false
        }
      ]
    },
    {
      id: 4,
      name: "IJK",
      groupName: "Đối tác",
      checked: false,
      expand: false,
      amount_checked: 0,
      child: [
        {
          id: 4.1,
          name: "Funny",
          checked: false
        }
      ]
    },
    {
      id: 5,
      name: "KLM",
      checked: false,
      groupName: "Giao dịch",
      expand: false,
      amount_checked: 0,
      child: [
        {
          id: 5.1,
          name: "Edit",
          checked: false
        },
        {
          id: 5.2,
          name: "Delete",
          checked: false
        }
      ]
    }
  ];

  roles: any[] = [];
  constructor() { }

  ngOnInit() {
  }

  expand(item){
    item.expand = !item.expand;
  }

  addItem(item){
    var index = this.roles.findIndex(el => item == el);
    if(index == -1){
      this.roles.push(item);
    }
    else{
      this.roles[index].checked = item.checked;
    }
  }

  checkChild(item,childItem){
    if(childItem.checked) {
      item.amount_checked -= 1;
    }
    else {
      item.amount_checked += 1;
    }
    childItem.checked = !childItem.checked;
    this.addItem(childItem);
  }

  check(item){
    if (item.checked){
      item.amount_checked = 0;
      item.child.forEach(elem => {
        elem.checked = true;
      });
    }
    else {
      item.amount_checked = item.child.length;
      item.child.forEach(elem => {
        elem.checked = false;
      });
    }
    item.checked = !item.checked;
    item.child.forEach(elem => {
      elem.checked = !elem.checked;
      this.addItem(elem);
    });
    
  }

}
