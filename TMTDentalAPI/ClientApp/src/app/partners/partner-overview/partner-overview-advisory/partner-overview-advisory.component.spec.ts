import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerOverviewAdvisoryComponent } from './partner-overview-advisory.component';

describe('PartnerOverviewAdvisoryComponent', () => {
  let component: PartnerOverviewAdvisoryComponent;
  let fixture: ComponentFixture<PartnerOverviewAdvisoryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerOverviewAdvisoryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerOverviewAdvisoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
