import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TaiDateRangeFilterDropdownComponent } from './tai-date-range-filter-dropdown.component';

describe('TaiDateRangeFilterDropdownComponent', () => {
  let component: TaiDateRangeFilterDropdownComponent;
  let fixture: ComponentFixture<TaiDateRangeFilterDropdownComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TaiDateRangeFilterDropdownComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TaiDateRangeFilterDropdownComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
