import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TaiDateRangeSimpleDropdownComponent } from './tai-date-range-simple-dropdown.component';

describe('TaiDateRangeSimpleDropdownComponent', () => {
  let component: TaiDateRangeSimpleDropdownComponent;
  let fixture: ComponentFixture<TaiDateRangeSimpleDropdownComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TaiDateRangeSimpleDropdownComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TaiDateRangeSimpleDropdownComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
