import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerTitleListComponent } from './partner-title-list.component';

describe('PartnerTitleListComponent', () => {
  let component: PartnerTitleListComponent;
  let fixture: ComponentFixture<PartnerTitleListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerTitleListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerTitleListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
