import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PayrollStructureCreateUpdateComponent } from './payroll-structure-create-update.component';

describe('PayrollStructureCreateUpdateComponent', () => {
  let component: PayrollStructureCreateUpdateComponent;
  let fixture: ComponentFixture<PayrollStructureCreateUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PayrollStructureCreateUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PayrollStructureCreateUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
