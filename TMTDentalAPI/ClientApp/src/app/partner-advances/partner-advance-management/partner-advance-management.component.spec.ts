import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerAdvanceManagementComponent } from './partner-advance-management.component';

describe('PartnerAdvanceManagementComponent', () => {
  let component: PartnerAdvanceManagementComponent;
  let fixture: ComponentFixture<PartnerAdvanceManagementComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerAdvanceManagementComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerAdvanceManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
