import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PayrollStructureListComponent } from './payroll-structure-list.component';

describe('PayrollStructureListComponent', () => {
  let component: PayrollStructureListComponent;
  let fixture: ComponentFixture<PayrollStructureListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PayrollStructureListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PayrollStructureListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
