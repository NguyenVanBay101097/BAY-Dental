import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'app-tai-search-input',
  templateUrl: './tai-search-input.component.html',
  styleUrls: ['./tai-search-input.component.css']
})
export class TaiSearchInputComponent implements OnInit {
  @Input() search: string;
  searchUpdate = new Subject<string>();
  @Output() searchChange = new EventEmitter<string>();
  @Output() upDownEnterChange = new EventEmitter<string>();
  constructor() { }

  ngOnInit() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.searchChange.emit(value);
      });
  }

  onKeydown(e) {
    if (e.keyCode == 40 || e.keyCode == 38 || e.keyCode == 13) {
      this.upDownEnterChange.emit(e.keyCode);
    }
  }

}
