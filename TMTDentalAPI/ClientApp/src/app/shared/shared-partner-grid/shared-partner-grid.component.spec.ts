import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SharedPartnerGridComponent } from './shared-partner-grid.component';

describe('SharedPartnerGridComponent', () => {
  let component: SharedPartnerGridComponent;
  let fixture: ComponentFixture<SharedPartnerGridComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SharedPartnerGridComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SharedPartnerGridComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
