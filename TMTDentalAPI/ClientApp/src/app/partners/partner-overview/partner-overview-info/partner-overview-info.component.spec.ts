import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerOverviewInfoComponent } from './partner-overview-info.component';

describe('PartnerOverviewInfoComponent', () => {
  let component: PartnerOverviewInfoComponent;
  let fixture: ComponentFixture<PartnerOverviewInfoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerOverviewInfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerOverviewInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
