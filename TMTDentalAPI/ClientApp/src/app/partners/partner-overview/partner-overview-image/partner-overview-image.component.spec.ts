import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerOverviewImageComponent } from './partner-overview-image.component';

describe('PartnerOverviewImageComponent', () => {
  let component: PartnerOverviewImageComponent;
  let fixture: ComponentFixture<PartnerOverviewImageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerOverviewImageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerOverviewImageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
