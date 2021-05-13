import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DateRangeFilterDropdownComponent } from './date-range-filter-dropdown.component';

describe('DateRangeFilterDropdownComponent', () => {
  let component: DateRangeFilterDropdownComponent;
  let fixture: ComponentFixture<DateRangeFilterDropdownComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DateRangeFilterDropdownComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DateRangeFilterDropdownComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
