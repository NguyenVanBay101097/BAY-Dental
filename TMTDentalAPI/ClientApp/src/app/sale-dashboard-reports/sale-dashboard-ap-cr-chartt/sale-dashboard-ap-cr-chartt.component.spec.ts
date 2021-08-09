import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleDashboardApCrCharttComponent } from './sale-dashboard-ap-cr-chartt.component';

describe('SaleDashboardApCrCharttComponent', () => {
  let component: SaleDashboardApCrCharttComponent;
  let fixture: ComponentFixture<SaleDashboardApCrCharttComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleDashboardApCrCharttComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleDashboardApCrCharttComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
