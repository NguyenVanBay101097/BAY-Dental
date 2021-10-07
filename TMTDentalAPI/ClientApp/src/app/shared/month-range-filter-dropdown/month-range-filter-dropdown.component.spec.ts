import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MonthRangeFilterDropdownComponent } from './month-range-filter-dropdown.component';

describe('MonthRangeFilterDropdownComponent', () => {
  let component: MonthRangeFilterDropdownComponent;
  let fixture: ComponentFixture<MonthRangeFilterDropdownComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MonthRangeFilterDropdownComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MonthRangeFilterDropdownComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
