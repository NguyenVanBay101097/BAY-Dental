import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EmployeeAdvanceSearchComponent } from './employee-advance-search.component';

describe('EmployeeAdvanceSearchComponent', () => {
  let component: EmployeeAdvanceSearchComponent;
  let fixture: ComponentFixture<EmployeeAdvanceSearchComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EmployeeAdvanceSearchComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EmployeeAdvanceSearchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
