import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerOtherInfoComponent } from './partner-other-info.component';

describe('PartnerOtherInfoComponent', () => {
  let component: PartnerOtherInfoComponent;
  let fixture: ComponentFixture<PartnerOtherInfoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerOtherInfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerOtherInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
