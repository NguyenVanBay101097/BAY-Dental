import { Component, OnInit, ɵConsole } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';


@Component({
  selector: 'app-cash-book',
  templateUrl: './cash-book.component.html',
  styleUrls: ['./cash-book.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class CashBookComponent implements OnInit {
  href: string;
  type: string;

  constructor(
    private router: Router, 
    private route: ActivatedRoute
  ) { }

  ngOnInit() {
    this.href = this.router.url.split("?")[0].split("/")[2];
    this.type = this.route.parent.snapshot.queryParams['type'];
  }

  
}
